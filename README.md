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

# MVP:
- Välja frontend ramverk
- Enkel textbox och element för att visa svar från Gemma3
- Skicka inputen från textbox till backend
- Skicka tillbaka svar från Gemma3 upp till frontend
- VIsa svaret i frontend
