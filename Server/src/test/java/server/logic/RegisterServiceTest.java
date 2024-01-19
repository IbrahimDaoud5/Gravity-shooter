package server.logic;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.dao.DataAccessException;
import org.springframework.jdbc.core.JdbcTemplate;

import static org.mockito.Mockito.when;
import static org.mockito.ArgumentMatchers.any;
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
        when(jdbcTemplate.queryForObject(any(String.class), any(Object[].class), any(Class.class)))
                .thenReturn(0); // User does not exist
        when(jdbcTemplate.update(any(String.class), any(Object[].class)))
                .thenReturn(1); // 1 row affected

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
}
