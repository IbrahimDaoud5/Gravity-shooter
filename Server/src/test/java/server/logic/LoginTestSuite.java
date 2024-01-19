
package server.logic;

import org.junit.platform.suite.api.SelectClasses;
import org.junit.platform.suite.api.Suite;

@Suite
@SelectClasses({LoginServiceTest.class, LoginControllerTest.class})
public class LoginTestSuite {
    //REMAINS EMPTY
}
