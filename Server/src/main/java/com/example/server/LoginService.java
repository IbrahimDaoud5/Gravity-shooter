package com.example.server;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.RowMapper;
import org.springframework.stereotype.Service;

import java.sql.ResultSet;
import java.sql.SQLException;

@Service
public class LoginService {

    @Autowired
    private JdbcTemplate jdbcTemplate;

    public String login(String username, String password) {
        String sql = "SELECT count(*) FROM users WHERE username = ? AND password = ?";
       // Integer count = jdbcTemplate.queryForObject(sql, Integer.class, new Object[]{username, password});
        Integer count = jdbcTemplate.queryForObject(
                sql,
                new Object[]{username, password},
                new RowMapper<Integer>() {
                    public Integer mapRow(ResultSet rs, int rowNum) throws SQLException {
                        return rs.getInt(1);
                    }
                }
        );

        if (count != null && count > 0) {
            return "Login successful";
        } else {
            return "Invalid username or password";
        }
    }
}
