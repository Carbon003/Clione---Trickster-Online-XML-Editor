# Clione - Trickster Online XML Editor

![Platform](https://img.shields.io/badge/platform-Windows-blue)
![Framework](https://img.shields.io/badge/.NET-8.0-purple)
![Status](https://img.shields.io/badge/status-Beta-orange)

**Clione** is a Windows-based XML editor made for **Trickster Online separated XML-based server files**.

It is designed to help server owners and developers edit Trickster Online game configuration files more easily through a table/grid interface instead of manually editing raw XML.

This project is inspired by **PyroSamurai's TO-Toolbox**, but focuses specifically on XML-based separated server setups.

---

## Important Compatibility Notice

Clione is **not made for S2 servers**.

This tool is intended for **separated XML-based Trickster Online server structures**, such as servers using the newer separated XML configuration format, including setups related to **Paula Trickster**.

If your server does not use this XML-based structure, Clione may not work correctly.

---

## What It Does

Clione allows you to open a folder containing Trickster Online XML configuration files and edit them through a simple Windows Forms interface.

Current features include:

* Open a folder containing `.xml` files
* List all XML files found in the selected folder
* Display XML table data in a grid view
* Edit XML values directly from the grid
* Automatically save changes when cells are edited
* Search/filter XML files by filename
* Search inside the currently loaded table
* Jump through search matches by pressing Enter
* Clone selected rows
* Rename XML files safely
* Delete XML files
* Show selected XML file in Windows Explorer
* Display loaded XML count
* Display row count for the selected table
* Basic dark-themed interface

---

## Expected XML Format

Clione expects XML files using a table-like structure similar to:

```xml
<TABLE>
  <FIELDINFO Name="id" />
  <FIELDINFO Name="name" />
  <FIELDINFO Name="value" />

  <ROW>
    <id>1</id>
    <name>Example</name>
    <value>100</value>
  </ROW>
</TABLE>
```

The editor reads the `FIELDINFO` entries as columns and the `ROW` entries as editable data rows.

---

## Features Not Implemented Yet

Some menu items and planned features are not fully implemented yet.

Not implemented or incomplete:

* Manual save settings
  Changes are currently saved automatically.
* Log viewer
* Create new table
* Edit columns
* Preferences/settings menu
* Schema-aware validation
* Advanced XML validation before saving
* Support for non-table XML structures
* Support for S2 server formats
* Support for LibConfig/server files outside the separated XML format
* Undo/redo system
* Backup system before saving edits
* Import/export tools
* Bulk editing tools

---

## Requirements

* Windows
* .NET 8.0 Desktop Runtime
* A Trickster Online separated XML-based server setup

---

## How To Use

1. Launch Clione.
2. Click **Open**.
3. Select the folder containing your Trickster Online XML files.
4. Choose an XML file from the list on the left.
5. Edit values directly in the table.
6. Changes are saved automatically after editing a cell.

---

## Recommended Usage

Before editing files, make a backup of your XML folder.

Clione directly modifies the selected XML files when values are changed. Since the project is still in beta, keeping backups is strongly recommended.

---

## Project Status

Clione is currently in **Beta**.

It already works for basic XML table editing, but several advanced features are still planned or incomplete.

---

## Credits

Inspired by **PyroSamurai's TO-Toolbox**.

Created for the Trickster Online private server community, especially those working with separated XML-based server files.

---

## Disclaimer

This tool is provided as-is.

Always back up your files before editing. The author is not responsible for broken server files, corrupted XML data, or lost configuration changes caused by improper use.
