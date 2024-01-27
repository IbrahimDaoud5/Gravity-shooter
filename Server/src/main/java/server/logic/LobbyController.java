package server.logic;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;


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
}
