package server.logic;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.dao.DataAccessException;
import org.springframework.jdbc.core.JdbcTemplate;

import java.sql.SQLIntegrityConstraintViolationException;

import static org.mockito.Mockito.when;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.junit.jupiter.api.Assertions.*;

/**
 * Tests for {@link RegisterService}.
 * This class uses Mockito to simulate the behavior of JdbcTemplate for various scenarios.
 */
@ExtendWith(MockitoExtension.class)
public class RegisterServiceTest {

    @Mock
    private JdbcTemplate jdbcTemplate;

    @InjectMocks
    private RegisterService registerService;

    /**
     * Test successful registration.
     */
    @Test
    public void whenRegisterNewUser_thenSuccess() {
        // Mock to return 0 for any String and Object[] arguments, simulating that the user does not exist
        when(jdbcTemplate.queryForObject(any(String.class), any(Object[].class), eq(Integer.class))).thenReturn(0);

        // Mock to return 1 for the specific SQL string and specific username and password arguments
        when(jdbcTemplate.update(eq("INSERT INTO users (username, password) VALUES (?, ?)"), eq("newuser"), eq("password"))).thenReturn(1);

        String result = registerService.register("newuser", "password");
        assertEquals("Registration successful", result);
    }



    /**
     * Test registration failure when user already exists.
     */
    @Test
    public void whenRegisterExistingUser_thenUserExists() {
        when(jdbcTemplate.queryForObject(any(String.class), any(Object[].class), any(Class.class)))
                .thenReturn(1); // User exists

        String result = registerService.register("existinguser", "password");
        assertEquals("User already exists", result);
    }

    /**
     * Tests the scenario where registration fails due to zero rows being affected.
     * This could occur in a situation where the user doesn't already exist,
     * but the registration insert operation fails to modify any rows in the database.
     */
    @Test
    public void checkRegistrationFailure() {
        // Mock to return 0 for any String and Object[] arguments, simulating that the user does not exist
        when(jdbcTemplate.queryForObject(any(String.class), any(Object[].class), eq(Integer.class))).thenReturn(0);

        // Mock to return 0 for the specific SQL string and specific username and password arguments
        // to simulate the scenario where the user doesn't exist, but the registration insert operation fails
        when(jdbcTemplate.update(eq("INSERT INTO users (username, password) VALUES (?, ?)"), eq("newuser"), eq("password"))).thenReturn(0);

        String result = registerService.register("newuser", "password");
        assertEquals("Registration failed", result);
    }


    /**
     * Test exception handling in registration process.
     */
    @Test
    public void whenDataAccessException_thenRethrow() {
        when(jdbcTemplate.queryForObject(any(String.class), any(Object[].class), any(Class.class)))
                .thenThrow(new DataAccessException("DB Error") {});

        assertThrows(DataAccessException.class, () ->
                registerService.register("user", "password")
        );
    }


    /**
     * Test the registration process when a SQLIntegrityConstraintViolationException is thrown.
     * This test simulates a scenario where a user attempts to register with a username that already exists,
     * violating a database integrity constraint. The test ensures that the service correctly interprets this
     * exception and informs the user that the username is already taken.
     */
    @Test
    public void whenSQLIntegrityConstraintViolation_thenUserAlreadyExists() {
        // Create a DataAccessException with SQLIntegrityConstraintViolationException as its cause
        DataAccessException dae = new DataAccessException("Constraint violation") {
            @Override
            public Throwable getCause() {
                return new SQLIntegrityConstraintViolationException();
            }
        };

        // Mock the queryForObject call to simulate the user not existing
        when(jdbcTemplate.queryForObject(any(String.class), any(Object[].class), eq(Integer.class))).thenReturn(0);

        // Set up the jdbcTemplate.update to throw the DataAccessException
        when(jdbcTemplate.update(eq("INSERT INTO users (username, password) VALUES (?, ?)"), eq("existinguser"), eq("password"))).thenThrow(dae);

        String result = registerService.register("existinguser", "password");
        assertEquals("User already exists", result);
    }


    /**
     * Tests the handling of a generic runtime exception during the registration process.
     * This test ensures that the service correctly propagates unexpected runtime exceptions.
     */
    @Test
    public void whenGenericException_thenRethrow() {
        when(jdbcTemplate.queryForObject(any(String.class), any(Object[].class), any(Class.class)))
                .thenThrow(new RuntimeException("Generic Error"));

        assertThrows(RuntimeException.class, () ->
                registerService.register("user", "password")
        );
    }

}
