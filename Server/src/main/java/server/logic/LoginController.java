package server.logic;


import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;
/**
 * Controller class for handling user login requests.
 * This class provides a REST endpoint for user authentication.
 */
@RestController
public class LoginController {

    @Autowired
    private LoginService loginService;
    /**
     * Handles the HTTP POST request for user login.
     * Delegates the authentication process to the LoginService and
     * returns a response entity with the authentication outcome message.
     *
     * @param user A User object containing the username and password submitted for login.
     * @return A ResponseEntity containing a message about the authentication outcome.
     *         The message can indicate either a successful login ("Login successful")
     *         or a failure ("Invalid username or password").
     */
    @PostMapping("/login")
    public ResponseEntity<String> login(@RequestBody User user) {
        String message = loginService.login(user);
        return ResponseEntity.ok(message);
    }
    @PostMapping("/logout")
    public ResponseEntity<String> logout(@RequestBody User user) {
        String message = loginService.logout(user.getUsername());
        return ResponseEntity.ok(message);
    }
}
