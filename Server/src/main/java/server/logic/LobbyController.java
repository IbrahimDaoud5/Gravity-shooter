package server.logic;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;


/*
@RestController
public class LobbyController {

    @Autowired
    private Lobby lobby;

    @PostMapping("/lobby")
    public ResponseEntity<String> login(@RequestBody User user) {
        String message = lobby.start(user.getUsername());
        return ResponseEntity.ok(message);
    }
}
*/