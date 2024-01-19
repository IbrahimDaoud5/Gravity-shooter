package server.logic;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.dao.DataAccessException;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Service;

import java.sql.SQLIntegrityConstraintViolationException;

/**
 * Service class for handling user registration.
 * This class interacts with the database to register new users.
 */
@Service
public class RegisterService {

    @Autowired
    private JdbcTemplate jdbcTemplate;

    /**
     * Registers a new user with the provided username and password.
     * It first checks if the user already exists in the database.
     * If not, it attempts to insert a new record with the username and password.
     *
     * @param username The username of the user to be registered.
     * @param password The password of the user to be registered.
     * @return A string message indicating the outcome of the registration attempt.
     *         Returns "User already exists" if the username is already taken,
     *         "Registration successful" if the user is successfully registered,
     *         or "Registration failed" if the registration process fails for other reasons.
     * @throws DataAccessException if a data access exception occurs, not related to SQLIntegrityConstraintViolationException.
     */
    public String register(String username, String password) {
        try{
        // Check if the user already exists
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
