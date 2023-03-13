import './BaseLayout.css'
import { FC, ReactElement, PropsWithChildren } from 'react'

interface Children { }
const LayoutBase: FC<PropsWithChildren<Children>> = (props: PropsWithChildren<Children>): ReactElement => {
    return(
        <div className='blok'>
            <div className='internal-block'>
                {props.children}
            </div>
        </div>
    )
}

export default  LayoutBase;