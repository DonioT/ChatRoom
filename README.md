# SignalR .net Chat 

The aim of this mini project was to figure out how to use signal r messaging for a web application.There are a ton of mini additions that can be made to make this a more useful chat application. 

I learned a couple of things during this project:

- Code first database approach, versus the database first approach I always used in the past

- signal r framework 

- improved jquery knowledge 

- Trying to use best practises such as (reusable views/partial views, user authentication{passing sensitive data through headers, password hashing etc}

## Known Problems / Improvements

- The front-end design of this web application is definitely lacking. It would be best to scrap the entire front end design and redo it to be more pleasing on the eye and responsive.

- Private message notifications don't update in real time if the user is already in the private message tab. There is no listener for that right now (only when the user switches tabs / logs in) 

- Group chatting can be implemented , the database tables and a few functions are designed for this case (but at the moment only 1 on 1 messaging is implemented), although I'm sure there is a lot I'm doing sub optimally.



## Screens

- [Login](https://imgur.com/HabnXcJ)
- [Register](https://imgur.com/IToqQAn)
- [Register avatar selection](https://imgur.com/bISiMAt)
- [Global Chat Room](https://imgur.com/ArPAHz0) > Clicking a user in this tab will take you to a private message with that user
- [Change avatar after registration](https://imgur.com/6r9KcnJ)
- [Private message tab](https://imgur.com/X8YjeOV) > On hover to delete a private message
- [New private chat in case user is offline](https://imgur.com/nTWBGtN) 
- [Basic validation example 1](https://imgur.com/IToqQAn) > Jgrowl notifactions used for success / errors
- [Basic validation example 2](https://imgur.com/7hYCm2S)





