#!/usr/bin/env python3
"""
Script to upload CSV files to MongoDB Atlas
"""

import os
import csv
import json
from pymongo import MongoClient
from datetime import datetime
import logging

# Setup logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# MongoDB connection string
MONGODB_URI = "mongodb+srv://kumarabhi6385_db_user:Rg98WQT2rbGk96bf@cluster0.0fx5rmx.mongodb.net/?appName=Cluster0"

# Database name
DATABASE_NAME = "SlashAlertDB"

# Directory containing CSV files
CSV_DIRECTORY = "/Users/abhisheksrivastava/WorkingDirectory/POCs/SlashAlert-POC/SlashAlert-API/SlashAlert/Database"

def connect_to_mongodb():
    """Connect to MongoDB Atlas"""
    try:
        client = MongoClient(MONGODB_URI)
        # Test the connection
        client.admin.command('ping')
        logger.info("Successfully connected to MongoDB Atlas")
        return client
    except Exception as e:
        logger.error(f"Failed to connect to MongoDB: {e}")
        return None

def convert_csv_row_to_document(row):
    """Convert CSV row to MongoDB document with proper data types"""
    document = {}
    
    for key, value in row.items():
        if value == "":
            document[key] = None
        elif value.lower() in ["true", "false"]:
            document[key] = value.lower() == "true"
        elif key.endswith(('_date', '_at', 'purchased_date')) and value:
            # Handle date fields first (before numeric check)
            try:
                # Try parsing ISO format
                if 'T' in value:
                    document[key] = datetime.fromisoformat(value.replace('Z', '+00:00'))
                elif value.count('-') == 2:  # Format like 2025-07-21
                    document[key] = datetime.strptime(value, '%Y-%m-%d')
                else:
                    document[key] = value  # Keep as string if not recognizable format
            except:
                document[key] = value  # Keep as string if parsing fails
        elif value.replace(".", "").replace("-", "").replace("+", "").isdigit():
            # Handle numbers (int or float) - improved to handle negative numbers
            if "." in value:
                try:
                    document[key] = float(value)
                except:
                    document[key] = value
            else:
                try:
                    document[key] = int(value)
                except:
                    document[key] = value
        else:
            document[key] = value
    
    return document

def upload_csv_to_collection(client, csv_file_path, collection_name):
    """Upload a single CSV file to MongoDB collection"""
    try:
        db = client[DATABASE_NAME]
        collection = db[collection_name]
        
        # Clear existing data (optional - remove this line if you want to append)
        collection.delete_many({})
        logger.info(f"Cleared existing data in {collection_name} collection")
        
        documents = []
        with open(csv_file_path, 'r', encoding='utf-8') as file:
            csv_reader = csv.DictReader(file)
            
            for row in csv_reader:
                document = convert_csv_row_to_document(row)
                documents.append(document)
                
                # Insert in batches of 1000
                if len(documents) >= 1000:
                    collection.insert_many(documents)
                    logger.info(f"Inserted {len(documents)} documents to {collection_name}")
                    documents = []
            
            # Insert remaining documents
            if documents:
                collection.insert_many(documents)
                logger.info(f"Inserted {len(documents)} documents to {collection_name}")
        
        # Get total count
        total_count = collection.count_documents({})
        logger.info(f"Successfully uploaded {csv_file_path} to {collection_name} collection. Total documents: {total_count}")
        
        return True
        
    except Exception as e:
        logger.error(f"Failed to upload {csv_file_path}: {e}")
        return False

def main():
    """Main function to upload all CSV files"""
    logger.info("Starting CSV to MongoDB upload process...")
    
    # Connect to MongoDB
    client = connect_to_mongodb()
    if not client:
        logger.error("Cannot proceed without MongoDB connection")
        return
    
    # Get all CSV files
    csv_files = []
    for file in os.listdir(CSV_DIRECTORY):
        if file.endswith('.csv') and not file.startswith('.'):
            csv_files.append(file)
    
    if not csv_files:
        logger.error(f"No CSV files found in {CSV_DIRECTORY}")
        return
    
    logger.info(f"Found {len(csv_files)} CSV files: {csv_files}")
    
    # Upload each CSV file
    successful_uploads = 0
    for csv_file in csv_files:
        csv_file_path = os.path.join(CSV_DIRECTORY, csv_file)
        
        # Generate collection name from file name (remove .csv and _export)
        collection_name = csv_file.replace('.csv', '').replace('_export', '')
        
        logger.info(f"Uploading {csv_file} to collection '{collection_name}'...")
        
        if upload_csv_to_collection(client, csv_file_path, collection_name):
            successful_uploads += 1
        else:
            logger.error(f"Failed to upload {csv_file}")
    
    # Close connection
    client.close()
    
    logger.info(f"Upload process completed. {successful_uploads}/{len(csv_files)} files uploaded successfully.")
    
    if successful_uploads == len(csv_files):
        logger.info("🎉 All CSV files uploaded successfully to MongoDB Atlas!")
    else:
        logger.warning(f"⚠️  {len(csv_files) - successful_uploads} files failed to upload.")

if __name__ == "__main__":
    main()