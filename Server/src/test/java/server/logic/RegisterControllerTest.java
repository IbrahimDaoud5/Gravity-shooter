package server.logic;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.springframework.http.MediaType;
import org.springframework.test.web.servlet.MockMvc;
import org.springframework.test.web.servlet.setup.MockMvcBuilders;
import org.mockito.junit.jupiter.MockitoExtension;
import static org.mockito.BDDMockito.given;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.content;
import com.fasterxml.jackson.databind.ObjectMapper;

/**
 * Tests for {@link RegisterController}.
 * This class uses MockMvc to simulate HTTP requests and verify the responses.
 */
@ExtendWith(MockitoExtension.class)
public class RegisterControllerTest {

    private MockMvc mockMvc;

    @Mock
    private RegisterService registerService;

    @InjectMocks
    private RegisterController registerController;

    @BeforeEach
    public void setup() {
        mockMvc = MockMvcBuilders.standaloneSetup(registerController).build();
    }

    /**
     * Test the register endpoint for successful registration.
     */
    @Test
    public void whenRegister_thenReturnsSuccessMessage() throws Exception {
        User user = new User("newuser", "password");
        given(registerService.register(user.getUsername(), user.getPassword())).willReturn("Registration successful");

        mockMvc.perform(post("/register")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(asJsonString(user)))
                .andExpect(status().isOk())
                .andExpect(content().string("Registration successful"));
    }

    /**
     * Test the register endpoint when the user already exists.
     */
    @Test
    public void whenRegisterExistingUser_thenReturnsUserExistsMessage() throws Exception {
        User user = new User("existinguser", "password");
        given(registerService.register(user.getUsername(), user.getPassword())).willReturn("User already exists");

        mockMvc.perform(post("/register")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(asJsonString(user)))
                .andExpect(status().isOk())
                .andExpect(content().string("User already exists"));
    }

    private static String asJsonString(final Object obj) {
        try {
            return new ObjectMapper().writeValueAsString(obj);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
}
