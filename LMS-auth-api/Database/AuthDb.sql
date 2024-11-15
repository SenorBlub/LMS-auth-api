CREATE DATABASE auth_service_db;

USE auth_service_db;

CREATE TABLE refresh_tokens (
    id CHAR(36) PRIMARY KEY,  
    token VARCHAR(512) NOT NULL,  
    user_id CHAR(36) NOT NULL,  
    expires_at DATETIME NOT NULL,  
    revoked TINYINT(1) DEFAULT 0,  
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP  
);

CREATE INDEX idx_user_id ON refresh_tokens(user_id);
