# Database Final Project

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)

## Introduction
This project is a basic server subscription manager for the web. It manages a backend SQLite database with EF-Core and a frontend built with Angular. The user can create an account, add a payment and subscribe to a subscription period. Any payments must be approved by an admin, who is created by manually changing the role of a user in the database from "User" to "Admin". The admin can also create subscription periods and view who is active for the current period.

## Features
- Cookie based user authentication and authorization
- Data entry and validation
- Advanced querying capabilities
- Transactions and isolation levels
- Backup and recovery

## Installation
1. Clone the repository:
    ```sh
    git clone https://github.com/gmoyer/db-finalproject.git
    ```
2. Navigate to the project directory:
    ```sh
    cd db-finalproject
    ```
3. Install dependencies:
    ```sh
    cd web
    npm install
    ```

## Usage
1. Start the frontend:
    ```sh
    cd web
    npm start
    ```
2. Start the backend:
    - a. Double click the `ServerSubscriptionManager/ServerSubscriptionManager.sln` file to open Visual Studio.
    - b. Click the play button to begin.
3. Open your browser and navigate to `http://localhost:3200`.
