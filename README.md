# AI-Butler

# How To Run:
1. Ensure you have installed .NET 9 or above [download](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
2. Download Ollama [here](https://ollama.com/download)
2. Open Command line 
   1. Win + R
   2. Type "cmd"
3. Type `ollama pull gemma3:1b` to install the model `gemma3:1b`
4. Type `ollama serve` to start a local ollama server
5. Go to the "AI-Butler" directory in your console and run `dotnet run --project Butler/Butler.ConsoleApp`

# Get database up and running:
1. Download Docker [here](https://www.docker.com/)
2. Open Command line 
   1. Win + R
   2. Type "cmd"
3. Go to the AI-Butler directory in the console
4. Run `docker compose up -d`. It will start the database and download the necessary packages
5. Check that the db is up and running with `docker ps`, should return 'butler-pg'
6. Test query to double check: run `docker exec -it butler-pg psql -U butler -d butler -c "SELECT 1;"`
7. Create pgvector-extension + table (only do this the first time).
     1. Run `docker exec -it butler-pg psql -U butler -d butler -c "CREATE EXTENSION IF NOT EXISTS vector;"`
     2. Run `docker exec -it butler-pg psql -U butler -d butler -c "
               CREATE TABLE IF NOT EXISTS rag_chunks (
                 id uuid PRIMARY KEY,
                 source text NOT NULL,
                 content text NOT NULL,
                 embedding vector(768) NOT NULL
   );"`   
   8. To stop the DB, run `docker compose down`. And start again with `docker compose up -d`

# MVP:
- Välja frontend ramverk
- Enkel textbox och element för att visa svar från Gemma3
- Skicka inputen från textbox till backend
- Skicka tillbaka svar från Gemma3 upp till frontend
- VIsa svaret i frontend
