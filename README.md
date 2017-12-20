# TwitchChatBot

Twitch Chat Bot, written for practice in C#.

Basic commands:

/addgame - allows to subs add any game to the list and to the html file with table, example: /addgame Don't Starve

/givetoall - give to all specified amount of points, command only for streamer, example: /givetoall 50

/donate - allows you to transfer points to other users, example: /donate Jepe34 100

/give - only for streamer. Allows to give some points to the specific user, example: /give Jepe34 100

/points - allows to see the number of your points

Every few minutes, the bot pings to the Twitch so that the Twitch does not interrupt the connection.

Also there are several spam filters, the commands can be used every few seconds. This is done to avoid unnecessary load on the bot, as well as flooding in the chat.
