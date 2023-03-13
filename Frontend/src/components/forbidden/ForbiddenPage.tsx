import './ForbiddenPage.css'
import { FC } from 'react';

const ForbiddenPage: FC = () => {
    return(
        <div className='app'>
            <div>403</div>
            <div className='txt'> Forbidden<span className='blink'>_</span> </div>
        </div>
    )
}

export default ForbiddenPage;