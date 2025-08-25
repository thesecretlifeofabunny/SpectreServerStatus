# SpectreServerStatus

This is a Console Application that produces a table of server uptime information by pinging the servers in your
config.json or as arguments. 


## Requirements
A computer with dotnet SDK 8 and a terminal

## Building
Git clone the repository locally. Then run either "dotnet run", or "dotnet publish --sc --ucr  --o your-output-directory-name-here"

## Running
### With a Configuration (Linux Only)
Have a file named config.json in the configuration directory named SpectreServerStatus  within XDG_CONFIG_DIRS specified directory 
or within your current ~/.config.
The config.json is the in the form of 

[<br>
<ol>"Server1",<br>
"Server2",<br>
...<br>
"ServerN"<br></ol>
]

run the binary produced by the build or by using "dotnet run"

### Without a Configuration
run the binary produced by the build or by using "dotnet run" followed by a space seperated list of servers.<br>
Example: "dotnet run server1 server2 ... serverN"


## Example Running Image
<img width="1387" height="228" alt="Screenshot_2025-08-21_21-38-31" src="https://github.com/user-attachments/assets/81918091-295f-481d-b04f-da2409215cb7" />
