import pandas as pd
import os

def convert_csv_to_excel():
    # Path to the CSV file
    csv_file = os.path.join(os.path.dirname(__file__), 'file_catalog_updated.csv')
    
    # Path to the new Excel file
    excel_file = os.path.join(os.path.dirname(__file__), 'Starkiller Base Command - File Catalog Updated.xlsx')
    
    # Read the CSV file
    df = pd.read_csv(csv_file)
    
    # Create a dictionary to hold DataFrames for each category
    category_dfs = {}
    
    # Group by category
    for category, group_df in df.groupby('Category'):
        category_dfs[category] = group_df.reset_index(drop=True)
    
    # Create a writer object
    with pd.ExcelWriter(excel_file, engine='openpyxl') as writer:
        # Add the introduction sheet
        intro_df = pd.DataFrame({
            'File Organization Strategy': [
                '',
                'When implementing new features or modifying existing functionality, refer to this catalog to understand which systems to modify.',
                '',
                'The catalog is organized by system categories:',
                '- Core System: Central game infrastructure components',
                '- Ship System: Ship generation and encounter handling',
                '- Family System: Imperial family management',
                '- Consequences System: Decision consequence tracking',
                '- Daily Game Flow: Daily game cycle management',
                '- Content Management: Media and game content',
                '- Visual Effects: Visual enhancement components',
                '',
                'This catalog was updated on ' + pd.Timestamp.now().strftime('%Y-%m-%d') + '.',
                'It includes newly added scripts and recent modifications.'
            ]
        })
        intro_df.to_excel(writer, sheet_name='Introduction', index=False)
        
        # Add a sheet for each category
        for category, cat_df in category_dfs.items():
            sheet_name = category.replace('System', 'System Files')
            if sheet_name == 'Core Files':
                sheet_name = 'Core System Files'
            if sheet_name == 'Visual Effects':
                sheet_name = 'Visual Effects Files'
            if sheet_name == 'Content Management':
                sheet_name = 'Content Management Files'
                
            cat_df.to_excel(writer, sheet_name=sheet_name, index=False)
    
    print(f"Excel file created at: {excel_file}")

if __name__ == "__main__":
    convert_csv_to_excel()