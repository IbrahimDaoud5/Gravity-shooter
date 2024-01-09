package com.example.server;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.dao.DataAccessException;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Service;

import java.sql.SQLIntegrityConstraintViolationException;

@Service
public class RegisterService {

    @Autowired
    private JdbcTemplate jdbcTemplate;

    public String register(String username, String password) {
        try{
        // Check if user already exists
        String sqlCheck = "SELECT count(*) FROM users WHERE username = ?";
        Integer count = jdbcTemplate.queryForObject(sqlCheck, new Object[]{username}, Integer.class);

        if (count != null && count > 0) {
            return "User already exists";
        }

        // Insert new user
        String sqlInsert = "INSERT INTO users (username, password) VALUES (?, ?)";
        int result = jdbcTemplate.update(sqlInsert, username, password);

        if (result > 0) {
            return "Registration successful";
        } else {
            return "Registration failed";
        }
    } catch (DataAccessException e) {
        if (e.getCause() instanceof SQLIntegrityConstraintViolationException) {
            return "User already exists";
        }
        throw e; // rethrow other exceptions
    }
    }
}
