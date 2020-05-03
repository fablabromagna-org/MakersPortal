# Makers Portal
## by FabLab Romagna
[![Build Status](https://dev.azure.com/fablabromagna/Makers%20Portal/_apis/build/status/fablabromagna-org.MakersPortal?branchName=master)](https://dev.azure.com/fablabromagna/Makers%20Portal/_build/latest?definitionId=1&branchName=master)

## Running in the local environment
If you are building an app or a web app you can easily run a docker image of the server.

## Why a lot of code of Azure Key Vault Extensions has been rewritten?
A lot of code has been rewritten because it is impossible to mock and correctly test our KeyManager.
If you know a better way to test the KeyManager class, please make a pull request or tell us more with an issue.