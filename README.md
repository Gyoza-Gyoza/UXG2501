# My assignment is due in 15 minutes so I resorted to using AI, it was insane

## Current Features  
  
- Basic chat function  
- Pulling responses from a csv file  
- Responses based on keywords  
- Weighted responses  
- Unlockable responses  
- Pulling from google sheets  
- Exact keyword matching  
 
## Database instructions  
  
### ID Naming convention  
U - Responses that do not require to be unlocked  
L - Responses that are locked behind other responses, can be unlocked by using the UnlocksResponse column  
  
E.g.  
| ResponseID | Keywords  | UnlocksResponse |
| --- | --- | --- |
| U0000006  | You are my sunshine  | L0000001 |
| L0000001  | You make me happy |  |
  
In this case, the system will lock Response ID L0000001  
  
As Response ID U0000006 has Response ID L0000001 under its UnlocksResponse column, when the user inputs the keywords of Response ID U0000006, the system will unlock ResponseID L0000001.  
  
### Keywords  
These are the words that the system will use to find a suitable response.  
Fill up the keywords as detailed as possible, the more detailed the keywords, the higher the chance of the response being used  
  
Adding the @ symbol instead of spaces will allow the system to recognize it as a Exact Phrase.  
Responses that have an Exact Phrase under their keyword column will only have their response used when:  
- All keywords are used  
- All keywords are in the stated order  
- There are no other words before, in between or after the keywords  
  
### Responses  
These are the responses that the system will use to respond to the user based on the keywords column. 
