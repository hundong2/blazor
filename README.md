# Blazor MAUI & MudBlazor: Searchable TreeView ComboBox Template

Welcome! This project demonstrates how to build a searchable TreeView ComboBox in a Blazor Hybrid MAUI application using the MudBlazor component library. It's designed to be a starting point for developers new to Blazor and MudBlazor.

This project was adapted from an existing structure to specifically showcase this custom component.

## Features

*   **Blazor Hybrid with .NET MAUI:** Run your Blazor components natively on desktop and mobile.
*   **MudBlazor Integration:** A beautiful Material Design component library, pre-configured and ready to use.
*   **Searchable TreeView ComboBox:** A custom-built component that allows users to select items from a hierarchical list with a powerful search/filter capability.

## How It Was Built (Development Steps)

This document outlines the steps taken to add the custom component to this project.

1.  **Project Analysis:** The initial step was to analyze the existing solution to identify the main MAUI project (`DataInspector.MAUI`) and the shared Razor component library (`DataInspector.SharedComponents`). It was discovered that MudBlazor was already installed but not fully configured.

2.  **Add MudBlazor Static Assets:** The required CSS and JavaScript files for MudBlazor were added to `DataInspectorApp/DataInspector.MAUI/wwwroot/index.html` to enable proper rendering of the components.

3.  **Create Data Models:** A simple C# class (`TreeItem.cs`) was created in `DataInspector.SharedComponents/Models` to represent the hierarchical data for the TreeView.

    ```csharp
    // DataInspector.SharedComponents/Models/TreeItem.cs
    public class TreeItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public HashSet<TreeItem> Children { get; set; } = new HashSet<TreeItem>();
        // ... constructors ...
    }
    ```

4.  **Build the TreeView ComboBox Component:** A new component, `TreeViewComboBox.razor`, was created in `DataInspector.SharedComponents/Components/`. This component encapsulates all the logic for rendering the tree, filtering nodes based on user input, and managing the selection. The filtering logic recursively builds a new tree containing only items that match the search term or have children that match.

5.  **Demonstrate the Component:** The new `TreeViewComboBox` was added to the main page (`DataInspector.SharedComponents/Pages/MainPage.razor`) with sample data to show how it works.

    ```razor
    // DataInspector.SharedComponents/Pages/MainPage.razor
    <TreeViewComboBox Label="Select a File or Folder"
                      Items="@_treeData"
                      @bind-SelectedItem="_selectedTreeItem" />

    <MudText Class="mt-4">Selected: @(_selectedTreeItem?.Name ?? "None")</MudText>
    ```

## TODO / Future Enhancements

Here are some ideas for extending this project:

*   **Dynamic Data Loading:** Modify the TreeView to load its data from an API instead of a hardcoded list.
*   **Advanced Selection Logic:** Implement logic to handle multi-selection or checkbox-based selection within the tree.
*   **State Persistence:** Save the last selected item in the browser's local storage and restore it on the next visit.
*   **Virtualization:** For very large trees, implement virtualization to only render the visible nodes, improving performance.
*   **Improve Styling:** Customize the theme further or add component-specific styles for a unique look and feel.

## References & Learning Resources

To understand the concepts used in this project, please refer to the following resources:

*   **Blazor Basics:**
    *   [Introduction to ASP.NET Core Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
    *   [Build a .NET MAUI Blazor Hybrid app](https://learn.microsoft.com/en-us/dotnet/maui/tutorials/blazor-hybrid)
*   **MudBlazor:**
    *   [Official Documentation](https://mudblazor.com/)
    *   [TreeView Component](https://mudblazor.com/components/treeview)
    *   [Popover Component](https://mudblazor.com/components/popover)
*   **C# & .NET:**
    *   [C# LINQ for data filtering](https://learn.microsoft.com/en-us/dotnet/csharp/linq/)

---
*This README is now complete.*
