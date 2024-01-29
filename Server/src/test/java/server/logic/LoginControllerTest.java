package server.logic;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.springframework.http.MediaType;
import org.springframework.test.web.servlet.MockMvc;
import org.springframework.test.web.servlet.setup.MockMvcBuilders;

import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.content;
import org.mockito.junit.jupiter.MockitoExtension;
import static org.mockito.BDDMockito.given;
import com.fasterxml.jackson.databind.ObjectMapper;
//
///**
// * The {@code LoginControllerTest} class contains tests for the {@code LoginController} class.
// * It uses MockMvc to simulate HTTP requests and verify the behavior of the login endpoint.
// */
//@ExtendWith(MockitoExtension.class)
//public class LoginControllerTest {
//
//    private MockMvc mockMvc;
//
//    @Mock
//    private LoginService loginService;
//
//    @InjectMocks
//    private LoginController loginController;
//
//    /**
//     * Sets up the testing environment before each test.
//     * It initializes the MockMvc instance with standalone setup for the controller under test.
//     */
//    @BeforeEach
//    public void setup() {
//        mockMvc = MockMvcBuilders.standaloneSetup(loginController).build();
//    }
//
//    /**
//     * Tests the {@code login} endpoint for a successful login scenario.
//     * It mocks the {@code loginService} to return a success message and verifies that
//     * the endpoint returns the correct HTTP status and response body.
//     */
//    @Test
//    public void whenLoginSuccess_thenReturns200AndSuccessMessage() throws Exception {
//        User user = new User("validUser", "validPass");
//        given(loginService.login(user.getUsername(), user.getPassword())).willReturn("Login successful");
//
//        mockMvc.perform(post("/login")
//                        .contentType(MediaType.APPLICATION_JSON)
//                        .content(asJsonString(user)))
//                .andExpect(status().isOk())
//                .andExpect(content().string("Login successful"));
//    }
//
//    /**
//     * Tests the {@code login} endpoint for a failed login scenario.
//     * It mocks the {@code loginService} to return a failure message and verifies that
//     * the endpoint returns the correct HTTP status and response body.
//     */
//    @Test
//    public void whenLoginFailure_thenReturns200AndFailureMessage() throws Exception {
//        User user = new User("invalidUser", "invalidPass");
//        given(loginService.login(user.getUsername(), user.getPassword())).willReturn("Invalid username or password");
//
//        mockMvc.perform(post("/login")
//                        .contentType(MediaType.APPLICATION_JSON)
//                        .content(asJsonString(user)))
//                .andExpect(status().isOk())
//                .andExpect(content().string("Invalid username or password"));
//    }
//
//    /**
//     * Helper method to convert an object into a JSON string representation.
//     * @param obj The object to be converted into JSON.
//     * @return A string containing the JSON representation of the object.
//     */
//    private static String asJsonString(final Object obj) {
//        try {
//            return new ObjectMapper().writeValueAsString(obj);
//        } catch (Exception e) {
//            throw new RuntimeException(e);
//        }
//    }
//}
