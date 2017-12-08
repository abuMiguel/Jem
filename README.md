# Jem
Lightweight NLP powered chatbot in C#

This project is in R&D and is intended to be an intelligent agent with Natural Language Understanding capabilities.
Currently, it includes a partially completed Part of Speech (POS) tagger and rudimentary sentence parsing capabilities. It
utilizes web scraping with ScrapySharp to dynamically load POS data for unknown words. That data will be stored in a SQLite
database to make subsequent requests faster and to enable offline parsing. The chatbot portion is in its infancy, but I am
developing an intent classification system to allow the bot to understand requests and respond in natural language. 
