package server.logic;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.RowMapper;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.mockito.Mockito.when;

/**
 * The {@code LoginServiceTest} class contains tests for the {@code LoginService} class.
 * It utilizes Mockito to mock the {@code JdbcTemplate} dependency and tests the login functionality.
 */
@ExtendWith(MockitoExtension.class)
public class LoginServiceTest {

    @Mock
    private JdbcTemplate jdbcTemplate;

    @InjectMocks
    private LoginService loginService;

    /**
     * Tests the {@code login} method for a successful login scenario.
     * It stubs the {@code queryForObject} method of the {@code JdbcTemplate} to return a count of 1,
     * simulating a successful find of the user in the database.
     * The method asserts that the result of the {@code login} method returns the expected success message.
     */
    @Test
    void testLoginSuccess() {
        // Setup the mock to return 1, simulating a user found in the database
        when(jdbcTemplate.queryForObject(
                eq("SELECT count(*) FROM users WHERE username = ? AND password = ?"),
                any(Object[].class),
                any(RowMapper.class)
        )).thenReturn(1);

        // Call the login method
        String result = loginService.login("abc", "123");

        // Assert the expected success message
        assertEquals("Login successful",result );
    }

    /**
     * Tests the {@code login} method for a failed login scenario.
     * It stubs the {@code queryForObject} method of the {@code JdbcTemplate} to return a count of 0,
     * simulating that the user was not found in the database.
     * The method asserts that the result of the {@code login} method returns the expected failure message.
     */
    @Test
    void testLoginFailure() {
        // Setup the mock to return 0, simulating no user found in the database
        when(jdbcTemplate.queryForObject(
                eq("SELECT count(*) FROM users WHERE username = ? AND password = ?"),
                any(Object[].class),
                any(RowMapper.class)
        )).thenReturn(0);

        // Call the login method with invalid credentials
        String result = loginService.login("invalidUser", "invalidPass");

        // Assert the expected failure message
        assertEquals("Invalid username or password", result);
    }
}
