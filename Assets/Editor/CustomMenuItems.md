# Starkiller Custom Unity Menu Items

## Top Menu Bar Items

### Starkiller Menu
- **Data Import**
  - Import CSV (CSVImporter.cs)
  - Import Excel (ExcelConverter.cs)
  - Import JSON Data (DataImporter.cs)
- **Data Management**
  - Generate Data Models (DataModelGenerator.cs)
  - Migrate Media Database (MediaDatabaseMigrationTool.cs)
  - Simple Media Migration (SimpleMediaMigrationTool.cs)
- **Setup**
  - Create Encounter System (EncounterSystemSetup.cs)
  - Create Game Manager
  - Create UI Manager
  - Create Audio Manager
  - Create Save Manager
  - Create Save/Load Manager
  - Full Project Setup
- **Tools**
  - Data Tools (StarkkillerDataTools.cs)
  - Create Render Textures (CreateRenderTextures.cs)
  - Starkiller Systems Manager (StarkkillerSystemsEditor.cs)

### Starkiller Base Menu
- **Test Editor Window** (TestEditorWindow.cs)
- **Data Editor** (ImperialDataEditor.cs) - *Fixed: Moved to Editor folder*

## Create Menu Items (Project Window Right-Click)

### Assets > Create > Starkiller
- **Encounters**
  - Encounter Media Manager (EncounterMediaManagerEditor.cs)
- **Decision Template** (DecisionTemplateEditor.cs)

## File Locations
- Main Editor Scripts: `Assets/_scripts/Editor/`
- Additional Editor Scripts: `Assets/Editor/`
- Third-party Editor Scripts: Various plugin folders (properly located)

## Troubleshooting
If menus don't appear after fixing file locations:
1. Right-click Assets folder → Reimport All
2. Check Console for compilation errors
3. Restart Unity Editor
4. Ensure no duplicate script names exist