# Breaker
A Permission & User management system.

Currently only supports use over the console due to the limited support of addons in s&box.

# Using it as an addon
1. Press the "Addon" button when creating a server
2. Search for `amper.breaker`
3. Select the addon
4. Done! You should now be able to use the addon.

# Integrating with a gamemode
If you are a gamemode developer and want better integration with your gamemode (Chat messages, custom commands, custom selectors, etc)
you will have to include all the code in your repository either as a GitHub Submodule or just by downloading the code. 
Which one you should use depends on your situation but for stability its recommended to not use Submodules.

# Things to keep in mind
There are a currently a lot of restrictions put in place by s&box which lead to the following things you should be aware of.
- Data is stored Per-Organisation and thus will require you to set up all your groups and users per Organisation.
  In the future we are hoping to add either a global file storage solution or a way of hosting a database for Breaker to connect to.
- You might lose performance because of the hacks which needed to be used to get some basic features to work,
  if your gamemode is already struggling performance wise you probably shouldnt be using this for now.
- The Built-In featureset is currently pretty limited, there is basically only support for basic moderation features which will be enough for most cases
  so if you want to add your own commands, feel free to fork this addon and maybe create a Pull request if you feel a lot of people will find your new commands useful! :)
