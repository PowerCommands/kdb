# KDB - Knowledge database
KDB is a very simple console application using the command line framework Power Commands.
The "database" is a simple Json file with links to the actual knowledge, the links could be urls, one note links (One Note of course needed), paths to either a file or a directory.
# Commands

## ```add```
```add <Type> "<link to item>" --name "<Name>" --tags <tag1>,<tag2>```
### Type
Type should be one of the following types: **url** **onenote** **path** **file**
### Link
Link to the actual knowledge, example ```https://learn.microsoft.com/en-us/archive/msdn-magazine/msdn-magazine-issues``` for a **url** type.
### --name
Option name with the following value, example ```-- name "Microsoft MSDN magazine issues"
### --tags
--tags if optional but it helps you find the item later when you search as it also is searchable just as the link and name is.
Example ```--tags newspaper,it,download```

https://github.com/PowerCommands/kdb/assets/102176789/6df39a21-33df-40eb-9b83-48067c2a6341

## ```find```
Find is the default command so you actually do not need to type it. You could just go ahead and input your search phrases as many as you want, every phrase act as an filter, for example ```microsoft news``` will be first search every thing matching ```microsoft``` and continue with that search result and match everything with the phrase ```news```.
Of course ```find microsoft news``` works just as well. 
### --Year
You could use a year filter on the search result using ```--year``` option like this: ```find microsoft --year 2023```.
### --Month
You could narrow you search even more with month filter on the search result using ```--month``` option like this: ```find microsoft --year 2023 --month 7``` notice that year must also be provided.

https://github.com/PowerCommands/kdb/assets/102176789/5da5386a-3454-4a77-b6be-8cfe64d032cf

## ```latest```
With latest you could create a search result based on date instead av search phrases.
### --days
Number of days from now, example show latest post the last seven days.
```latest --days 7```
### --weeks
Number of weeks from now, example show latest post the last two weeks.
```latest --weeks 2```
### --url --path --onenote --file
Type filter, only one can be used, example show every onenote document the last 30 days.
```latest --days 30 --onenote```
## **Handle the result**
After you have used ```find``` or ```latest``` to get a search result you need to select one item before you could run any of the other commands for ```edit```, ```delete``` or ```open```.
Notice that the last selected item is always remembered by the application, so if you do searches after that without selecting anything, ```edit``` will open the last selected item for edit.

![Alt text](images/selected_item.png?raw=true "Selected item")

## ```open``` // ```o``` // ```[CTRL] + [O]```
Open the selected item.
## ```edit```
Edit the selected item.
## ```delete```
Delete the selected item.
## ```tags```
Add tags    : ```tags <myNewTag>```

Remove tags : ```tags <myExistingDeleteTag> --delete```

## **Backup**
With ```backup``` command a backup of the database file will be stored in the configured directory in the ```PowerCommandsConfiguration.yaml``` file.

### --show
Use option ```--show``` to open the backup directory and handle the backup files.

## **Restore**
With ```restore``` you can restore your database using an backup created earlier, just run ```restore``` command and follow the instructions.

## **Db**
```db``` command gives you the possibility to open the database file with your favorite editor and hack the file manually, your editor must be configured in the ```PowerCommandsConfiguration.yaml``` file like this:

```codeEditor: C:\Users\%USERNAME%\AppData\Local\Programs\Microsoft VS Code\Code.exe```

## About OneNote
Notice that a onenote link requires you to have Microsoft OnoNote installed and you need to have set the path for it in the configuration file.
In the ```PowerCommandsConfiguration.yaml``` file and the section named shellConfiguration:
```
shellConfiguration:
    pathToOneNote: C:\Program Files\Microsoft Office\root\Office16\ONENOTE        
    autostart: true
```
