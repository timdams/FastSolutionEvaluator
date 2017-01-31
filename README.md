FastSolutionEvaluator
=====================

Little WPF tool to rapidly browse subdirectories containing csharp code (handy if you need to manually evaluate/compare a large stack of projects)

One usefull additional experimental branche exist:
	*ConsoleControl: utilizes a control to show output of compiled project (buggy: crashes when second program is run)

# Wishlist
* Countdown: shows how many projects are already evaluated 
* Dynamic checkboxes, based from XML file
* Read already evaluated results and parse to the checkboxes
* Redo datamodel of evaluation stuff
* Redo datamodel of project/solutions: now it's all file-based, but it'll be more robus if I parse the .sln/.csproj files and use that information
* Go for full mvvm (byebye "SelectedItem as SolutionMeta"
* Push F5 to start current project

