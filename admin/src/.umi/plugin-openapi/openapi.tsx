// @ts-nocheck

import SwaggerUI from 'swagger-ui-react';
import 'swagger-ui-react/swagger-ui.css';
import { Card } from 'antd';
      
const App = () => (
  <Card>
    <SwaggerUI url="/umi-plugins_openapi.json" />
  </Card>
);
export default App;
      