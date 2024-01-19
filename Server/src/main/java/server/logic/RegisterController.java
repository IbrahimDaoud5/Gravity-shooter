package server.logic;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;
/**
 * Controller class for handling user registration requests.
 * This class provides a REST endpoint for user registration.
 */
@RestController
public class RegisterController {

    @Autowired
    private RegisterService registerService;
    /**
     * Handles the HTTP POST request for user registration.
     * Delegates the registration process to the RegisterService and
     * returns a response entity with the registration outcome message.
     *
     * @param user A User object containing the username and password submitted for registration.
     * @return A ResponseEntity containing a message about the registration outcome.
     *         The message can indicate either success ("Registration successful"),
     *         failure due to existing user ("User already exists"),
     *         or other failures ("Registration failed").
     */
    @PostMapping("/register")
    public ResponseEntity<String> register(@RequestBody User user) {
        String message = registerService.register(user.getUsername(), user.getPassword());
        return ResponseEntity.ok(message);
    }
}
