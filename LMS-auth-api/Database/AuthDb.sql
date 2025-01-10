CREATE DATABASE authdb;

USE authdb;

CREATE TABLE refresh_tokens (
    id VARCHAR(36) PRIMARY KEY,  
    token VARCHAR(512) NOT NULL,  
    user_id VARCHAR(36) NOT NULL,  
    expires_at DATETIME NOT NULL,  
    revoked TINYINT(1) DEFAULT 0,  
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP  
);

