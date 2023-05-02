import classes from './CommonButton.module.css'
import { FC, ReactElement } from 'react';
import Button, { ButtonProps } from 'react-bootstrap/Button';

export interface IButtonProps{
    children: ReactElement[];
    props: ButtonProps
}

/// Компонента с общестпользуемой кнопкой.
export const CommonButton: FC<IButtonProps> = (props: IButtonProps): ReactElement => {
    return (
    <Button {...props.props} className={classes.commonButton}>
        {props.children}
    </Button>
    );
}

