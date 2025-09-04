#!/usr/bin/env python3
# Excel to CSV Converter for Starkiller Base Command
import pandas as pd
import os
import sys
import re

def clean_filename(filename):
    """Clean a filename to be safe for filesystems"""
    # Replace spaces with underscores
    cleaned = filename.replace(' ', '_')
    # Remove any non-alphanumeric characters (except underscores)
    cleaned = re.sub(r'[^\w]', '', cleaned)
    return cleaned

def convert_excel_to_csv(excel_file, output_folder):
    """ Convert Excel file to CSV, one file per sheet """
    try:
        # Create output folder if it doesn't exist
        if not os.path.exists(output_folder):
            os.makedirs(output_folder)
            
        # Read Excel file
        excel = pd.ExcelFile(excel_file)
        sheet_names = excel.sheet_names
        
        # Convert each sheet to CSV
        for sheet in sheet_names:
            df = pd.read_excel(excel, sheet_name=sheet)
            
            # Clean up the data - handle NaN values
            df = df.fillna('')
            
            # Create a safe filename
            safe_sheet_name = clean_filename(sheet)
            csv_file = os.path.join(output_folder, f'{safe_sheet_name}.csv')
            
            # Save to CSV
            df.to_csv(csv_file, index=False, encoding='utf-8')
            print(f'Converted sheet "{sheet}" to {csv_file}')
            
        return True, f'Successfully converted {len(sheet_names)} sheets to CSV'
    except Exception as e:
        return False, f'Error converting Excel to CSV: {str(e)}'

if __name__ == '__main__':
    excel_file = '/Users/ig-macbookpro/Downloads/GameData.xlsx'
    output_folder = '/Users/ig-macbookpro/Documents/Personal/Starkiller/Starkiller Base Command/Assets/_Temp/CSV'
    
    success, message = convert_excel_to_csv(excel_file, output_folder)
    
    if success:
        print(message)
        sys.exit(0)
    else:
        print(message, file=sys.stderr)
        sys.exit(1)