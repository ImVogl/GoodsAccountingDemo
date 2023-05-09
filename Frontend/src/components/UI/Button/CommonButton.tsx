import classes from './CommonButton.module.css'
import { FC, ReactElement } from 'react';
import Button from 'react-bootstrap/Button';

export interface IButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement>{
    title: string;
    variant: string;
    href?: string;
}

/// Компонента с общестпользуемой кнопкой.
export const CommonButton: FC<IButtonProps> = ({ title, variant, href, ...props }): ReactElement => {
    return (
    <Button {...props} variant={variant} href={href} className={classes.commonButton}>
        {title}
    </Button>
    );
}

