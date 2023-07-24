# KDB - Knowledge database
KDB is a very simple console application using the command line framework Power Commands.
The "database" is a simple Json file with links to the actual knowledge, the links could be urls, one note links (One Note of course needed), paths to either a file or a directory.

https://github.com/PowerCommands/Publishing-System/assets/102176789/b023c337-fc64-43a5-b1d6-c606b4f2bcdf

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



## About OneNote
Notice that a onenote link requires you to have Microsoft OnoNote installed and you need to have set the path for it in the configuration file.
In the ```PowerCommandsConfiguration.yaml``` file and the section named shellConfiguration:
```
shellConfiguration:
    pathToOneNote: C:\Program Files\Microsoft Office\root\Office16\ONENOTE        
    autostart: true
```