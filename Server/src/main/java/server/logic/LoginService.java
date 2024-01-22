package server.logic;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.RowMapper;
import org.springframework.stereotype.Service;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

/**
 * Service class for handling user login.
 * This class interacts with the database to authenticate users.
 */
@Service
public class LoginService {

    @Autowired
    private JdbcTemplate jdbcTemplate;


    // Map for storing active user sessions and their ready status
    public static final ConcurrentHashMap<String, User> activeUsers = new ConcurrentHashMap<>();


    public void printActiveSessions() {
        for (Map.Entry<String, User> entry : activeUsers.entrySet()) {
            String username = entry.getKey();
            User user = entry.getValue();
            // Assuming User has a meaningful toString() implementation
            System.out.println("Username: " + username + ", User Info: " + user);
        }
        System.out.println("*************************************************************");
    }


    /**
     * Authenticates a user based on the provided username and password.
     * It checks the database for a user record matching the username and password.
     *
     * @param user The user attempting to log in.
     * @return A string message indicating the outcome of the login attempt.
     * Returns "Login successful" if credentials match an existing user,
     * or "Invalid username or password" if authentication fails.
     */
    public String login(User user) {
        printActiveSessions();
        String sql = "SELECT count(*) FROM users WHERE username = ? AND password = ?";
        Integer count = jdbcTemplate.queryForObject(
                sql,
                new Object[]{user.getUsername(), user.getPassword()},
                new RowMapper<Integer>() {
                    public Integer mapRow(ResultSet rs, int rowNum) throws SQLException {
                        return rs.getInt(1);
                    }
                }
        );

        if (count != null && count > 0) {
            user.setReady(false);
            synchronized (activeUsers) {
                if (!activeUsers.containsKey(user.getUsername())) {
                    activeUsers.put(user.getUsername(), user);
                    return "Login successful";
                }
                else return "User is already logged in";
            }
        }
        else return "Invalid username or password";
    }

    // Method to handle user logout
    public String logout(String username) {
        User user = activeUsers.get(username);
        if (user != null) {
            activeUsers.remove(username);
        }
        return "logged out successfully";
    }
}