import './SellPage.css'
import { FC, useState } from 'react';
import { ButtonGroup, Button, Col } from 'react-bootstrap';

import WorkingArea from '../base/working/WorkingArea';
import SellPageCurrent from './SellPageCurrent';

const CURRENT: string = "current";
const PREVIOUS: string = "previous";

const SellPageRoot: FC = () =>{
    const [key, setKey] = useState(CURRENT);
    const component = key === CURRENT ? <SellPageCurrent /> : <></>
    return(
        <WorkingArea>
            <div className='sell-page-root'>
                <ButtonGroup className='sell-page-tabs'>
                    <Button  className={key === CURRENT ? 'sell-page-tab active' : 'sell-page-tab'} onClick={() => setKey(CURRENT)}>Текущая</Button>
                    <Button  className={key === PREVIOUS ? 'sell-page-tab active' : 'sell-page-tab'} onClick={() => setKey(PREVIOUS)}>Прошлые</Button>
                    <Col/>
                </ButtonGroup>
                <div className='internal-sell-page-root'>
                    {component}
                </div>
            </div>
        </WorkingArea>
    );
}

export default SellPageRoot;
