# FontAwesomeJson

This Tool will take FontAwesome's YAML File and will create a JSON file, you can use as a Dictionary if you want to use FA in a desktop Application

The produced JSON File will have this format:
```json
{
"iconName" :
    {
    "Id" : "icon",
    "Styles" : ["style1", "style2"]
    }
}
```
Where the styles can be "regular", "solid" or "brands", corresponding to the 3 font files.
