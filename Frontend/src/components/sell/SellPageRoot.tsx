import './SellPage.css'
import { FC, useState } from 'react';
import { ButtonGroup, Button, Col } from 'react-bootstrap';

import { useAppSelector } from '../../common/redux/hooks';
import { selectUserIsAdmin } from '../../common/redux/UserSlice';
import WorkingArea from '../base/working/WorkingArea';
import SellPageCurrent from './SellPageCurrent';
import SellPagePrevious from './SellPagePrevious';
import SellAdminHistory from './SellAdminHistory';

const CURRENT: string = "current";
const PREVIOUS: string = "previous";

const SellPageRoot: FC = () =>{
    const [key, setKey] = useState(CURRENT);
    const admin = useAppSelector(selectUserIsAdmin);
    const previous = admin ? <SellAdminHistory /> : <SellPagePrevious />;
    const component = key === CURRENT ? <SellPageCurrent /> : previous;
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
