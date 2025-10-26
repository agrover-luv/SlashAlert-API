#!/usr/bin/env python3

import pymongo
from urllib.parse import quote_plus
import sys

def test_mongodb_connection():
    # Connection string from appsettings.json - updated with proper TLS settings
    connection_string = "mongodb+srv://kumarabhi6385_db_user:Rg98WQT2rbGk96bf@cluster0.0fx5rmx.mongodb.net/?retryWrites=true&w=majority&ssl=true&tlsInsecure=false&appName=Cluster0"
    database_name = "SlashAlertDB"
    
    try:
        print("Testing MongoDB Atlas connection...")
        print(f"Connection string: {connection_string}")
        print(f"Database name: {database_name}")
        print("-" * 50)
        
        # Create MongoDB client
        client = pymongo.MongoClient(connection_string)
        
        # Test the connection by pinging the server
        print("Attempting to ping MongoDB server...")
        client.admin.command('ping')
        print("✅ Successfully connected to MongoDB!")
        
        # Get database
        db = client[database_name]
        print(f"✅ Connected to database: {database_name}")
        
        # List collections
        collections = db.list_collection_names()
        print(f"📋 Available collections: {collections}")
        
        # Test each collection
        for collection_name in collections:
            try:
                collection = db[collection_name]
                count = collection.count_documents({})
                print(f"📊 Collection '{collection_name}': {count} documents")
            except Exception as e:
                print(f"❌ Error accessing collection '{collection_name}': {e}")
        
        client.close()
        return True
        
    except pymongo.errors.ServerSelectionTimeoutError as e:
        print(f"❌ Server selection timeout: {e}")
        print("This usually means:")
        print("1. Network connectivity issues")
        print("2. Incorrect connection string")
        print("3. MongoDB Atlas cluster is paused or unavailable")
        print("4. IP address not whitelisted in MongoDB Atlas")
        return False
        
    except pymongo.errors.AuthenticationFailed as e:
        print(f"❌ Authentication failed: {e}")
        print("This usually means:")
        print("1. Incorrect username or password")
        print("2. User doesn't have proper permissions")
        return False
        
    except pymongo.errors.ConfigurationError as e:
        print(f"❌ Configuration error: {e}")
        print("This usually means:")
        print("1. Invalid connection string format")
        print("2. Missing required connection parameters")
        return False
        
    except Exception as e:
        print(f"❌ Unexpected error: {e}")
        print(f"Error type: {type(e).__name__}")
        return False

if __name__ == "__main__":
    success = test_mongodb_connection()
    sys.exit(0 if success else 1)