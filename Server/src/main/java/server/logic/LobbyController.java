package server.logic;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;



@RestController
public class LobbyController {

    @Autowired
    private Lobby lobby;

    @PostMapping("/lobby")
    public ResponseEntity<String> lobby(@RequestBody String username) {
        String message = lobby.start(username);
        return ResponseEntity.ok(message);
    }
}
