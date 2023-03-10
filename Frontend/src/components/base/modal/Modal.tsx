import './Modal.css'
import React, { FC, ReactElement, PropsWithChildren } from 'react'

interface ModalArguments{
    active:boolean;
    setActive: Function;
}

const Modal: FC<PropsWithChildren<ModalArguments>> = ( props: PropsWithChildren<ModalArguments>): ReactElement => {
    return(
        <div className={props.active ? 'modal-background active' : 'modal-background'} onClick={() => props.setActive(false)}>
            <div className={props.active ? 'modal-content active' : 'modal-content'} onClick={e => e.stopPropagation()}>
                {props.children}
            </div>
        </div>
    );
};

export default Modal;
