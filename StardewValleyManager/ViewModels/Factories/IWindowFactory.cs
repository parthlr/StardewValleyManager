using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace StardewValleyManager.ViewModels.Factories;

public interface IWindowFactory<T> where T : Window
{
    void CreateWindow();

    void CreateWindow(object param);
}
