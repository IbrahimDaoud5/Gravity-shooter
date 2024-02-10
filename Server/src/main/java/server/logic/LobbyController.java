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
        return ResponseEntity.ok(message);
    }

    @PostMapping("/checkInvite")
    public ResponseEntity<String> checkInvite(@RequestBody User user) {
        String inviter = lobbyService.checkInvitation(user.getUsername());
        if (!inviter.isEmpty()) {

            return ResponseEntity.ok("You have an invitation from " + inviter);
        }
        return ResponseEntity.ok("No invitations");
    }
    @PostMapping("/sendInvite")
    public ResponseEntity<String> sendInvite(@RequestBody InviteRequestDto inviteRequest) {
        String fromUsername = inviteRequest.getFromUsername();
        String toUsername = inviteRequest.getToUsername();
        String message = lobbyService.sendInvitation(fromUsername, toUsername);
        return ResponseEntity.ok(message);
    }


    // Static inner class for DTO
    public static class InviteRequestDto {
        private String fromUsername;
        private String toUsername;

        // Getters and setters
        public String getFromUsername() {
            return fromUsername;
        }

        public void setFromUsername(String fromUsername) {
            this.fromUsername = fromUsername;
        }

        public String getToUsername() {
            return toUsername;
        }

        public void setToUsername(String toUsername) {
            this.toUsername = toUsername;
        }
    }
}
