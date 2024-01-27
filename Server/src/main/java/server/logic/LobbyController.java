package server.logic;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;


@RestController
@RequestMapping("/lobby")
public class LobbyController {

    @Autowired
    private LobbyService lobbyService;

    @GetMapping("/isReady")
    public ResponseEntity<String> lobby(@RequestBody String username) {
        String message = lobbyService.isReady(username);
        return ResponseEntity.ok(message);
    }
}
