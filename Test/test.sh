#!/bin/bash

# Navigate to test folder (update path if different)
cd BlogAPI.Tests

# Clean bin and obj
rm -rf bin obj

# Rebuild and run tests
dotnet clean
dotnet test --logger "console;verbosity=normal"