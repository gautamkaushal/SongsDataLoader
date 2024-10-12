App
- Console app
- consumes CSV file to generate SQLite *.db file for consumption
- may want to delete the previous versions of the *.db file before every run


To run
- ensure all the entries in CSV file with decimal time format entries are removed
- configure the connection string for the SQLite DB (just need the output path where the *.db file is to be stored) //Data folder can be used for it
- configure the batch size for writing to DB
- Run the app
