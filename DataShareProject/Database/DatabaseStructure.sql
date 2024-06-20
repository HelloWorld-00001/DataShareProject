-- Check if the DataShareDB database exists and drop it if it does
DROP DATABASE IF EXISTS DataShareDB;

-- Create the DataShareDB database
CREATE DATABASE DataShareDB;
USE DataShareDB;

-- Create the Account table
CREATE TABLE account (
    id INT AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(255) NOT NULL UNIQUE,
    password VARCHAR(256) NOT NULL,
    salt VARCHAR(256) NOT NULL,
    createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create the TextStore table
CREATE TABLE TextStore (
    id INT AUTO_INCREMENT PRIMARY KEY,
    content TEXT NOT NULL,
    createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    autoDelete bool not null,
    `Owner` INT,
    FOREIGN KEY (`Owner`) REFERENCES account(id)
);

-- Create the FileStore table
CREATE TABLE FileStore (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fileName VARCHAR(255) NOT NULL,
    fileData LONGBLOB NOT NULL,
    uploadedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    fileSize int not null,
    autoDelete bool not null,
    `Owner` INT,
    FOREIGN KEY (`Owner`) REFERENCES account(id)
);

-- jwt refresh token
CREATE TABLE RefreshToken (
    id INT AUTO_INCREMENT PRIMARY KEY,
    token VARCHAR(255) NOT NULL UNIQUE,
    createTime DATETIME NOT NULL,
    expiredTime DATETIME,
    userId INT NOT NULL,
    FOREIGN KEY (userId) REFERENCES `account`(`id`)
);

-- Insert a few sample records into the account table
INSERT INTO account (email, password, salt) VALUES ('test@example.com', '123', '123');
