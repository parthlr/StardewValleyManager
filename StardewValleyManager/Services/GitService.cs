using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace StardewValleyManager.Services;
public class GitService
{
    private string _authToken = "<token>";
    private string _user = "parthlr";
    private GitHubClient _client;

    private static GitService instance = null;

    public GitService()
    {
        _client = new GitHubClient(new ProductHeaderValue("StardewValleyManager"));
    }

    public static GitService Instance
    {
        get
        {
            if (instance == null)
            {
                return new GitService();
            }

            return instance;
        }
    }

    public string AuthToken
    {
        get => _authToken;
    }

    public string User
    {
        get => _user;
    }

    public GitHubClient Client
    {
        get => _client;
    }

    public void Authenticate()
    {
        Credentials credentials = new Credentials(AuthToken);
        Client.Credentials = credentials;
    }

    public async void CheckAndCreateRepository(string RepositoryName)
    {
        IReadOnlyCollection<Repository> repos = await Client.Repository.GetAllForUser(User);

        bool repoExists = false;
        foreach (Repository r in repos)
        {
            if (r.Name.Equals(RepositoryName))
            {
                repoExists = true;
                break;
            }
        }

        if (!repoExists)
        {
            NewRepository newRepository = new NewRepository(RepositoryName)
            {
                AutoInit = true,
                Description = "",
                Private = false
            };
            Repository context = await Client.Repository.Create(newRepository);
            System.Diagnostics.Debug.WriteLine($"New repository {RepositoryName} was created for user {User}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Repository {RepositoryName} already exists for user {User}");
        }
    }

    public async Task CommitAllSaves(SaveInfoModel[] Saves)
    {
        foreach (SaveInfoModel save in Saves)
        {
            await CommitSaveFolder(save.Name);
        }
    }

    public async Task<Reference> CommitSaveFolder(string SaveName)
    {
        System.Diagnostics.Debug.WriteLine($"Commiting save {SaveName}");
        string repo = "test-repo2";

        Commit latestCommit = await GetLatestCommit(repo);

        string filesLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/StardewValley/Saves/" + SaveName;
        string[] saveFiles = Directory.GetFiles(filesLocation);
        TreeResponse commitTree = await CreateCommitTree(repo, saveFiles, SaveName);

        DateTime currentDate = DateTime.Now;
        string locality = "en-US";
        Reference newCommit = await CreateNewCommit(repo, $"Saved {SaveName} on {currentDate.ToString(new CultureInfo(locality))}", latestCommit, commitTree);
        System.Diagnostics.Debug.WriteLine($"Finished saving {SaveName}");

        return newCommit;
    }

    public async Task<IReadOnlyList<GitHubCommit>> GetCommitHistory(string RepositoryName, string SaveName)
    {
        CommitRequest commitRequest = new CommitRequest { Path = $"{SaveName}/{SaveName}" };
        IReadOnlyList<GitHubCommit> commits = await Client.Repository.Commit.GetAll(User, RepositoryName, commitRequest);

        return commits;
    }

    public async Task<string> GetCommitContent(string RepositoryName, string FileName, string CommitSha)
    {
        IReadOnlyList<RepositoryContent> content = await Client.Repository.Content.GetAllContentsByRef(User, RepositoryName, FileName, CommitSha);

        string fileSha = content[0].Sha;

        Blob blob = await Client.Git.Blob.Get(User, RepositoryName, fileSha);
        string blobContent = blob.Content;

        byte[] rawTextData = Convert.FromBase64String(blobContent);
        string decodedText = Encoding.UTF8.GetString(rawTextData);

        return decodedText;
    }

    public async Task<Commit> GetLatestCommit(string RepositoryName)
    {
        string branchHead = "heads/master";

        Reference headReference = await Client.Git.Reference.Get(User, RepositoryName, branchHead);
        Commit latestCommit = await Client.Git.Commit.Get(User, RepositoryName, headReference.Object.Sha);

        return latestCommit;
    }

    private async Task<BlobReference> CreateFileBlob(string RepositoryName, string FileName)
    {
        string currentDate = DateTime.Now.ToString(new CultureInfo("en-US"));
        string fileHeader = $"<!--{currentDate}-->";

        string fileContent = fileHeader + "\n" + File.ReadAllText(FileName);

        NewBlob fileBlob = new NewBlob { Encoding = EncodingType.Utf8, Content = fileContent };
        BlobReference blobReference = await Client.Git.Blob.Create(User, RepositoryName, fileBlob);

        return blobReference;
    }

    private async Task<TreeResponse> CreateCommitTree(string RepositoryName, string[] Files, string SaveName)
    {
        Commit latestCommit = await GetLatestCommit(RepositoryName);
        NewTree commitTree = new NewTree { BaseTree = latestCommit.Tree.Sha };

        foreach (string file in Files)
        {
            if (File.Exists(file))
            {
                string commitFile = file.Replace(@"\", "/");
                System.Diagnostics.Debug.WriteLine(commitFile);
                BlobReference fileBlob = await CreateFileBlob(RepositoryName, commitFile);

                string fileName = Path.GetFileName(file);
                System.Diagnostics.Debug.WriteLine($"{SaveName}/{fileName}");

                NewTreeItem fileTreeItem = new NewTreeItem { Path = ($"{SaveName}/{fileName}"), Mode = "100644", Type = TreeType.Blob, Sha = fileBlob.Sha };
                commitTree.Tree.Add(fileTreeItem);
            }
        }

        TreeResponse tree = await Client.Git.Tree.Create(User, RepositoryName, commitTree);

        return tree;
    }

    private async Task<Reference> CreateNewCommit(string RepositoryName, string CommitMessage, Commit LatestCommit, TreeResponse Tree)
    {
        NewCommit newCommit = new NewCommit(CommitMessage, Tree.Sha, LatestCommit.Sha);

        Commit commitResponse = await Client.Git.Commit.Create(User, RepositoryName, newCommit);

        string branchHead = "heads/master";
        Reference savedReference = await Client.Git.Reference.Update(User, RepositoryName, branchHead, new ReferenceUpdate(commitResponse.Sha));

        return savedReference;
    }
}
