package server.logic;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.Arrays;


@RestController
@RequestMapping("/lobby")
public class LobbyController {

    @Autowired
    private LobbyService lobbyService;

    @PostMapping("/setReady")
    public ResponseEntity<String> lobby(@RequestBody User u) {
        String message = lobbyService.setToReady(u.getUsername());
        return ResponseEntity.ok(message);
    }
    @PostMapping("/logout")
    public ResponseEntity<String> logout(@RequestBody User u) {
        String message = lobbyService.logout(u.getUsername());
        return ResponseEntity.ok(message);
    }
    @PostMapping("/showConnectedUsers")
    public ResponseEntity<String[]> showConnectedUsers(@RequestParam(required = false) String query) {
        String[] message = lobbyService.showConnectedUsers(query);
        System.out.println(Arrays.toString(message));
        return ResponseEntity.ok(message);
    }
}
