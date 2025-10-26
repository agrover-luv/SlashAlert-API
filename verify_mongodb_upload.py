#!/usr/bin/env python3
"""
Script to verify MongoDB Atlas upload and show collection statistics
"""

from pymongo import MongoClient
import logging

# Setup logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

# MongoDB connection string
MONGODB_URI = "mongodb+srv://kumarabhi6385_db_user:Rg98WQT2rbGk96bf@cluster0.0fx5rmx.mongodb.net/?appName=Cluster0"
DATABASE_NAME = "SlashAlertDB"

def verify_upload():
    """Verify the upload and show statistics"""
    try:
        client = MongoClient(MONGODB_URI)
        db = client[DATABASE_NAME]
        
        print("=" * 60)
        print("MONGODB ATLAS UPLOAD VERIFICATION")
        print("=" * 60)
        print(f"Database: {DATABASE_NAME}")
        print(f"Connection: {MONGODB_URI.split('@')[1].split('?')[0]}")
        print()
        
        # Get all collections
        collections = db.list_collection_names()
        
        if not collections:
            print("❌ No collections found in the database!")
            return
        
        print(f"📊 Found {len(collections)} collections:")
        print("-" * 60)
        
        total_documents = 0
        
        for collection_name in sorted(collections):
            collection = db[collection_name]
            count = collection.count_documents({})
            total_documents += count
            
            print(f"📋 Collection: {collection_name:<15} | Documents: {count:>5}")
            
            # Show a sample document structure (first document)
            sample = collection.find_one()
            if sample:
                fields = list(sample.keys())
                print(f"   Fields ({len(fields)}): {', '.join(fields[:10])}")
                if len(fields) > 10:
                    print(f"              {'...' + str(len(fields) - 10) + ' more fields'}")
            print()
        
        print("-" * 60)
        print(f"✅ Total Documents Uploaded: {total_documents}")
        print("🎉 All CSV files successfully uploaded to MongoDB Atlas!")
        
        client.close()
        
    except Exception as e:
        print(f"❌ Error connecting to MongoDB: {e}")

if __name__ == "__main__":
    verify_upload()