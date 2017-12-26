FastSolutionEvaluator
=====================

Little WPF tool to rapidly browse subdirectories containing csharp code (handy if you need to manually evaluate/compare a large stack of projects)

One usefull additional experimental branche exist:
	*ConsoleControl: utilizes a control to show output of compiled project (buggy: crashes when second program is run)

# Bugs
Moving folder to new location (or opening on other pc, but same dropbox) will kill the connections. Folder path saved in .csv file should be relative to this file.

Currently doesn't work with VS2017 projects. See wishlist point one for possible fix ;)
	
# Wishlist
* Move project loading to roslyn code analyzer?
* Fix bug when loading other folder after having already opened one
* Take regular backups of evalscoresfiles
* Sommige dubbel tonen als tester maar onder random naam (uit lijst). Indien te harde afwijking tegenover de originele , de tester waarschuwen 
* Icon
* Redo datamodel of evaluation stuff
* Go for full mvvm (byebye "SelectedItem as SolutionMeta"
* Add "comments" textbox in evaluation part and also write it to the file
* Search function: in project, or in all projects. Highlight searched keyword and scroll to code immediatelly
* Basic duplicate-code searcher 
* SLDUpFinder integration + keep database
* Also allow solutionless projects to be added to the list.
* Posibility to map folder to studentname (and remember that mapping) in case a student f$$$ed up his foldernaming
* Use GUID as main identifcation (primary key), just make sure no duplicates exist. 
- Add option to ignore GUID to previous part in case teacher is correcting a solution that has same GUID (because he gave a template solution to start with for example)
* Highlight code to annotate (and save in resulting evaluation file)

# Done from wishlist
* Select correct file/project when clicking on other solution (now shows files from previous selection)
* Countdown: shows how many projects are already evaluated 
* Redo datamodel of project/solutions: now it's all file-based, but it'll be more robus if I parse the .sln/.csproj files and use that information
* Add "open in explorer" button for each folder
* Push F5 to start current project
* Dynamic checkboxes/evaluation, based from XML file
* Read already evaluated results and parse to the checkboxes