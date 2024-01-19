package server.logic;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.RowMapper;
import org.springframework.stereotype.Service;

import java.sql.ResultSet;
import java.sql.SQLException;
/**
 * Service class for handling user login.
 * This class interacts with the database to authenticate users.
 */
@Service
public class LoginService {

    @Autowired
    private JdbcTemplate jdbcTemplate;
    /**
     * Authenticates a user based on the provided username and password.
     * It checks the database for a user record matching the username and password.
     *
     * @param username The username of the user attempting to log in.
     * @param password The password of the user attempting to log in.
     * @return A string message indicating the outcome of the login attempt.
     *         Returns "Login successful" if credentials match an existing user,
     *         or "Invalid username or password" if authentication fails.
     */
    public String login(String username, String password) {
        String sql = "SELECT count(*) FROM users WHERE username = ? AND password = ?";
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
